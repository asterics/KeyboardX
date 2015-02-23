#!/usr/bin/bash

#
# minifies dev1, copy to keyboard.xsd, inject correct version and deploy to Player
#


../bin/minify.sh dev1-commented.xsd dev1.xsd
cp dev1.xsd keyboard.xsd

today=`date "+%Y%m%d"`
sed -i "/version=\"dev1\"/ s/dev1/$today/" keyboard.xsd

cp keyboard.xsd ../../Player/
