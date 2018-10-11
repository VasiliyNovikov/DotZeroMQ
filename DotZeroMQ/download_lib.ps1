cd $PSScriptRoot
$url = "https://github.com/zeromq/libzmq/releases/download/v4.2.5/zeromq-4.2.5.zip"
$output = "zeromq.zip"
Invoke-WebRequest -Uri $url -OutFile $output

Expand-Archive -Path zeromq.zip -DestinationPath .
mv zeromq-4.2.5 zeromq
rm zeromq.zip
