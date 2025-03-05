#!/usr/bin/env bash
if [ ! -f /usr/local/bin/ddnsclient/appsettings.Production.json ]; then
    APIKEY="$(whiptail --inputbox "Enter youre dynu apikey!" --title "APIKey" 8 78 "" 3>&1 1>&2 2>&3)"

    if [ -n "$APIKEY" ]; then
        sudo sh -c "echo '{ \"Ddns\": {\"APIKey\": \"$APIKEY\" } }' >> /usr/local/bin/ddnsclient/appsettings.Production.json"
    fi
fi
