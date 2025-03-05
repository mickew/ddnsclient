#!/usr/bin/env bash

#Check if script is being run as root
if [ "$(id -u)" != "0" ]; then
   echo "This script must be run as root" 1>&2
   exit 1
fi

if [ ! $? = 0 ]; then
   exit 1
else
   BASEDIR=${PWD}

   sh ./getlatest.sh

   chmod +x /usr/local/bin/ddnsclient/DdnsClient
   
   cp ddnsclient.service /etc/systemd/system
   if [ ! -f /etc/systemd/system/ddnsclient.service ]; then
     whiptail --title "Installation aborted" --msgbox "There was a problem writing the ddnsclient.service file" 8 78
    exit
   fi

   sh ./apikey.sh

   systemctl enable ddnsclient.service
   systemctl start ddnsclient.service
   whiptail --title "Installation complete" --msgbox "Dynamic DNS Client installation complete." 8 78

   #reboot
   #poweroff
fi
