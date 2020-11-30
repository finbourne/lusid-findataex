#!/bin/bash

echo $*
if [[ $* == *" -s "* ]]
then
  echo "running in safe mode"
fi
while getopts 'k:c:t:i:p:o:f:d:c:m:s:' opt
do
    echo "this opt is $opt"
    case $opt in
        m)
          echo setting max_instruments $OPTARG
          max_instruments=$OPTARG ;;
		    s)
          echo "setting safe_mode to true"
          echo "note passing in any text -s (including 'false') will flag safe mode. remove -s to turn off."
          safe_mode=true ;;
		    k)
          echo setting certificate_file $OPTARG
          certificate_file=$OPTARG ;;
        t)
          echo setting instrument_id_type $OPTARG
          instrument_id_type=$OPTARG ;;
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
        *)
          echo "unrecognised opt $OPTARG for $opt"
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

# decode the base64 encoded certificate file
pk12_cert_file=$(mktemp)
base64 -d $certificate_file > $pk12_cert_file

# set decoded certificate file path as env var for use by findataex
export BBG_DL_CERT=$pk12_cert_file

# if no filesystem provided set to lusid drive as default
if [[ -z $filesystem ]] ; then
  filesystem=Lusid
fi

# being constructing findataex cmd request
base_request="-t $instrument_id_type -o $output_dir -f $filesystem"

# check for the source of instruments to request (from portfolio or provided ids)
if [[ ! -z "$portfolios" ]]
then
  base_request="$base_request -p $portfolios"
elif [[ ! -z "$instrument_ids" ]]
then
  base_request="$base_request -i $instrument_ids"
else
  echo "Ensure portfolios or instrument_ids have been provided"
  exit 1
fi

if [[ ! -z "$data_fields" ]]
then
  base_request="dotnet $LUSID_FINDATA_EX_DLL getdata $base_request -d $data_fields"
elif [[ ! -z "$corp_actions" ]]
then
  base_request="dotnet $LUSID_FINDATA_EX_DLL getactions $base_request -c $corp_actions"
else
  echo "Ensure populated either data_fields to run getdata or corp_actions to run getactions"
  exit 1
fi

# check if max instruments have been passed in
if [[ ! -z "$max_instruments" ]]
then
  base_request="$base_request -m $max_instruments"
fi

# check if running in safe mode
if [[ ! -z "$safe_mode" ]]
then
  base_request="$base_request -s"
fi

echo "submitting findata ex request..."
echo $base_request

$base_request

echo "all arg vars for debug..."
echo findataexDll:$LUSID_FINDATA_EX_DLL
echo action:$action
echo portfolios:$portfolios
echo instrument_id_type :$instrument_id_type
echo instrument_ids:$instrument_ids
echo filesystem:$filesystem
echo output_dir:$output_dir
echo data_fields:$data_fields
echo corp_actions:$corp_actions
echo max_intruments:$max_instruments
echo safe_mode:$safe_mode


echo findataex request completed with exit code $?

# delete the tmp file for the identity file
trap "rm -f $tmp_file" EXIT
trap "rm -f $pk12_cert_file" EXIT
exit 0
