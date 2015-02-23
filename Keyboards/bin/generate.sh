#!/usr/bin/bash

#
# generates a grid with given dimensons full of images
#


# params

if [ $# != 3 ]; then
	echo "Usage: $(basename $0) <number of cols> <number of rows> <path to images>"
	exit 1
fi

X=$1
Y=$2
imgPath=${3%/}  # remove trailing slashes

if ! [ -d $imgPath ]; then
	echo "Path to images doesn't exist!"
	exit 1
fi

imgs=$(ls -1 $imgPath/{*.png,*.jpg,*.jpeg} 2> /dev/null)
num=$(echo "$imgs" | wc -l)

if [ "$(($X*$Y))" -gt "$num" ]; then
	echo "Too less images present for given dimension (${X}x${Y})!"
	exit 1
fi


# functions

# params:
# $1 - x dimension
# $2 - y dimension
function writeHead {
	echo "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
	echo ""
	echo "<!--"
	echo "DESCRIPTION"
	echo "-->"
	echo "<keyboard"
	echo "    xmlns=\"http://www.jku.at/iis/something\""
	echo "    xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\""
	echo "    xsi:noNamespaceSchemaLocation=\"../schema1.xsd\""
	echo "    version=\"20140828\""
	echo ">"
	echo "  <grid id=\"main\">"
	echo "    <dimension cols=\"$1\" rows=\"$2\" />"
}

# params:
# $1 - x dimension
# $2 - y dimension
# $3 - image path
function writeElement {
	echo "    <button id=\"btn$1-$2\">"
	echo "      <position x=\"$1\" y=\"$2\" />"
	echo "      <icon>{ICON_PREFIX}$(basename $3)</icon>"
	echo "    </button>"
}

function writeFoot {
	echo "  </grid>"
	echo "</keyboard>"
}


# main

writeHead $X $Y

x=0
y=0

for img in $imgs; do
	writeElement $x $y $img
	let x++
	if [ "$x" -eq "$X" ]; then
		x=0
		let y++
	fi
	if [ "$y" -eq "$Y" ]; then
		break
	fi
done

writeFoot

