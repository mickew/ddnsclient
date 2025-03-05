#!/usr/bin/env bash

#Check if script is being run as root
if [ "$(id -u)" != "0" ]; then
   echo "This script must be run as root" 1>&2
   exit 1
fi

if [ ! $? = 0 ]; then
   exit 1
else

   systemctl stop ddnsclient.service
   systemctl disable ddnsclient.service

   rm /etc/systemd/system/ddnsclient.service

   rm -f -r /usr/local/bin/ddnsclient

   whiptail --title "Uninstall complete" --msgbox "Dynamic DNS Client uninstall complete." 8 78
fi
