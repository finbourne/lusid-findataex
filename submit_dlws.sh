#!/bin/bash

# check for safe mode
if [[ $* == *" safemode"* ]]
then
  echo "running in safe mode..."
  safe_mode=true
fi

while getopts 'k:c:t:y:i:a:f:s:d:c:m:' opt
do
    echo "this opt is $opt"
    case $opt in
        m)
          echo setting max_instruments $OPTARG
          max_instruments=$OPTARG ;;
		k)
          echo setting certificate_file $OPTARG
          certificate_file=$OPTARG ;;
        t)
          echo setting instrument_id_type $OPTARG
          instrument_id_type=$OPTARG ;;
		y)
          echo setting yellowkey $OPTARG
          yellowkey=$OPTARG ;;
        i)
          echo setting instrument_source $OPTARG
          instrument_source=$OPTARG ;;
        a)
          echo setting insturments source arguments $OPTARG
          instrument_source_args=$OPTARG ;;
        f)
          echo setting output file name $OPTARG
          output_filename=$OPTARG ;;
		s)
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

# begin constructing findataex request
base_request="-t $instrument_id_type -f $output_filename"

# add yellowkey if provided to select a market sector for a TICKER type argument.
if [[ ! -z "$yellowkey" ]]
then
  base_request="$base_request -y $yellowkey"
fi

# check instrument source provided
if [[ ! -z "$instrument_source" ]]
then
  base_request="$base_request -i $instrument_source"
else
  echo "Ensure instrument_source has been provided"
  exit 1
fi

# add insturment source optional arguments
if [[ ! -z "$instrument_source_args" ]]
then
  base_request="$base_request -a $instrument_source_args"
fi

# add output filesystem
if [[ ! -z "$filesystem" ]]
then
  base_request="$base_request -s $filesystem"
fi

# populate data fields if pricing request (getdata action) or corp action types if corp action request (getactions)
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
  base_request="$base_request --safemode"
fi

echo "submitting findata ex request..."
echo $base_request
$base_request
dlws_exit_code=$?
echo "findataex request completed with exit code=$dlws_exit_code"

# delete the tmp file for the identity file
trap "rm -f $tmp_file" EXIT
trap "rm -f $pk12_cert_file" EXIT

# exit with same code as the request
exit $dlws_exit_code
