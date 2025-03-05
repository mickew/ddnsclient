#!/usr/bin/env bash

getlatesturl="https://raw.githubusercontent.com/mickew/ddnsclient/main/Tools/getlatest.sh"
installurl="https://raw.githubusercontent.com/mickew/ddnsclient/main/Tools/install.sh"
uninstallurl="https://raw.githubusercontent.com/mickew/ddnsclient/main/Tools/uninstall.sh"
updateurl="https://raw.githubusercontent.com/mickew/ddnsclient/main/Tools/update.sh"
ddnsclienturl="https://raw.githubusercontent.com/mickew/ddnsclient/main/Tools/ddnsclient.sh"
ddnsclientservice="https://raw.githubusercontent.com/mickew/ddnsclient/main/Tools/ddnsclient.service"

mkdir -p Tools
curl -o Tools/getlatest.sh $getlatesturl
curl -o Tools/install.sh $installurl
curl -o Tools/uninstall.sh $uninstallurl
curl -o Tools/update.sh $updateurl
curl -o Tools/rpidisplay.sh $ddnsclienturl
curl -o Tools/rpidisplay.service $ddnsclientservice

cd Tools/
sudo sh ddnsclient.sh
