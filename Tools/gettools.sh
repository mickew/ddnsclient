#!/usr/bin/env bash

getlatesturl="https://raw.githubusercontent.com/mickew/ddnsclient/main/Tools/getlatest.sh"
installurl="https://raw.githubusercontent.com/mickew/ddnsclient/main/Tools/install.sh"
uninstallurl="https://raw.githubusercontent.com/mickew/ddnsclient/main/Tools/uninstall.sh"
updateurl="https://raw.githubusercontent.com/mickew/ddnsclient/main/Tools/update.sh"
ddnsclienturl="https://raw.githubusercontent.com/mickew/ddnsclient/main/Tools/ddnsclient.sh"
ddnsclientservice="https://raw.githubusercontent.com/mickew/ddnsclient/main/Tools/ddnsclient.service"
ddnsapikey="https://raw.githubusercontent.com/mickew/ddnsclient/main/Tools/apikey.sh"

mkdir -p Tools
curl -o Tools/getlatest.sh $getlatesturl
curl -o Tools/install.sh $installurl
curl -o Tools/uninstall.sh $uninstallurl
curl -o Tools/update.sh $updateurl
curl -o Tools/ddnsclient.sh $ddnsclienturl
curl -o Tools/ddnsclient.service $ddnsclientservice
curl -o Tools/apikey.sh $ddnsapikey

cd Tools/
sudo sh ddnsclient.sh
