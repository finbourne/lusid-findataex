#!/bin/bash

while getopts 'k:a:c:t:i:p:o:f:d:c:' opt
do
    case $opt in
		k)
          echo setting certificate_file $OPTARG
          certificate_file=$OPTARG ;;
        a)
          echo setting action $OPTARG
          action=$OPTARG ;;
        t)
          echo setting intsrument_id_type $OPTARG
          intsrument_id_type=$OPTARG ;;
        i)
          echo setting instrument_ids $OPTARG
          instrument_ids=$OPTARG ;;
        p)
          echo setting portfolios $OPTARG
          portfolios=$OPTARG ;;
        o)
          echo setting output_dir $OPTARG
          output_dir=$OPTARG ;;
		f)
          echo setting output filesystem $OPTARG
          filesystem=$OPTARG ;;
        # program specific actions - standard data
        d)
          echo setting data_fields $OPTARG
          data_fields=$OPTARG ;;
        # program specific actions - corporate actions
        c)
          echo setting corp_actions $OPTARG
          corp_actions=$OPTARG ;;
    esac
done

# if no dll is provided then terminate
if [[ -z $LUSID_FINDATA_EX_DLL ]] ; then
  echo "Must set a findata ex runtime dll location through environment variable LUSID_FINDATA_EX_DLL"
  exit 1
fi

# if no certificate password is provided then terminate
if [[ -z $BBG_DL_PASS ]] ; then
  echo "Must set a BBG DataLicense certificate password through environment variable BBG_DL_PASS"
  exit 1
fi

# if the certificate_file is not passed as an arg use one from an env var or exit if not
if [[ -z $certificate_file ]] ; then
	echo No certificate file provided... attempting to use DLWS cert file from env var BBG_DL_CERT_BIN_STR
	if [[ -z $BBG_DL_CERT_BIN_STR ]] ; then
		echo No env varaible BBG_DL_CERT_BIN_STR is set. Exiting as no certificate for DLWS calls is available...
		exit 1
	fi
    tmp_file=$(mktemp)
cat >$tmp_file <<EOF
$BBG_DL_CERT_BIN_STR
EOF
    certificate_file=$tmp_file
fi

# check certificate file is not empty
if [[ ! -s $certificate_file ]] ; then
	echo "$certificate_file cannot be empty. Ensure a certificate file is passed in via -k or contents are set via the env variable BBG_DL_CERT_BIN_STR"
	exit 1
fi

echo $certificate_file output is...
cat $certificate_file
ls -ltr $certificate_file

# set certificate file path as env var for use by findataex
export BBG_DL_CERT=$certificate_file


# if no filesystem provided set to local
if [[ -z $filesystem ]] ; then
  filesystem=Lusid
fi

if [[ $action = "getdata" ]] && [[ ! -z $portfolios ]]
then
  dotnet $LUSID_FINDATA_EX_DLL getdata -p $portfolios -o ${0} -f $filesystem -d ${d}
elif [[ $action = "getdata" ]] &&  [[ ! -z $intsrument_id_type ]]
then
  dotnet $LUSID_FINDATA_EX_DLL getdata -t $intsrument_id_type -i $instrument_ids -o $output_dir  -f $filesystem -d $data_fields
elif [[ $action = "getaction" ]] &&  [[ ! -z $portfolios ]]
then
  dotnet $LUSID_FINDATA_EX_DLL getaction -p $portfolios -o $output_dir  -f $filesystem -c $corp_actions
elif [[ $action = "getaction" ]] &&  [[ ! -z $intsrument_id_type ]]
then
  dotnet $LUSID_FINDATA_EX_DLL getaction -t $intsrument_id_type -i $instrument_ids -o $output_dir  -f $filesystem -c $corp_actions
fi

# delete the tmp file for the identity file
trap "rm -f $tmp_file" EXIT

exit 0
