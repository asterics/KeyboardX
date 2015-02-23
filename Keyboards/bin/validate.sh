#!/usr/bin/bash

# validates all keyboards under 'showroom' and 'test' with definded schema


SCHEMA="dev1"

DIR="../schema"
SCHEMA_FILE="${DIR}/${SCHEMA}.xsd"
SCHEMA_COMMENTED="${DIR}/${SCHEMA}-commented.xsd"


if [ ! -r $SCHEMA_FILE ]; then
	echo "Error: Used schema file '${SCHEMA_FILE}' doesn't exist!"
	exit 1
fi

if [ -r $SCHEMA_COMMENTED ] && [ -r $SCHEMA_FILE ]; then
	# older than
	if [ $SCHEMA_FILE -ot $SCHEMA_COMMENTED ]; then
		echo "Info: Commented schema has changed - let's minify it."
		./minify.sh $SCHEMA_COMMENTED $SCHEMA_FILE
	fi
else
	echo "Warning: Commented schema file '${SCHEMA_COMMENTED}' doesn't exist!"
fi


echo -e "\nValidating keyboards in showroom..."
xmllint --noout --schema $SCHEMA_FILE ../showroom/*.xml

echo -e "\nValidating keyboards in test..."
xmllint --noout --schema $SCHEMA_FILE ../test/*.xml

echo -e "\nDone!"
