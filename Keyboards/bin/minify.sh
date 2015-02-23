#!/usr/bin/bash

# minifies a XML file by deleting all comments and empty lines

if [ $# -ne 2 ]; then
	echo "usage: $0 <input-file> <output-file>"
	exit 1
fi

# '/<!--.*-->/d'   deletes single line comments
# '/<!--/,/-->/d'  deletes multi line comments
# '/^\s*$/d'       deletes empty lines
sed '/<!--.*-->/d;/<!--/,/-->/d;/^\s*$/d' $1 > $2
