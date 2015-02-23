# KeyboardX

KeyboardX is a multi-purpose on-screen keyboard. It was designed with the needs of handicapped persons in mind. Due to its flexibility it can be used for several tasks though. Find more in [features](#features).

It was originally developed as a master thesis project for [Institut Integriert Studieren](http://jku.at/iis) at [JKU](http://jku.at). Currently it's hosted as an open source project by [Kompetenznetzwerk KI-I ](http://ki-i.at).

## Features

KeyboardX consists of two parts, the [player application](https://github.com/s3huber/KeyboardX/blob/master/Player) which is able to open a given [keyboard](https://github.com/s3huber/KeyboardX/blob/master/Keyboards/showroom) file.

Further a graphical editor for building keyboard files comfortably is planed.

### Keyboards

A keyboard is basically a XML file which is defined by XML Schema. A keyboard contains one or several grids. The player shows one grid at a time. Keyboards allow a very flexible structure.

### Player

Is written in C# for the Windows plattform and has the following features:
 - flexible configuration hierarchy
 - icon support (jpeg, png, bmp, svg, wmf)
 - scanning
   - different scanning mode per grid
   - linear scanning, column-row scanning, row-column scanning
   - configurable scanning times
 - actions
   - TCP command
   - text to speech

## Documentation

Please look at the [wiki](https://github.com/s3huber/KeyboardX/wiki) for documentation, examples and other information.

## Contribution

If you like or find this software useful, please consider to solve one or two [issues]( https://github.com/s3huber/KeyboardX/issues?q=is%3Aopen+is%3Aissue+label%3Atodo).

## License

KeyboardX is licensed under the GNU General Public License v2, see [LICENSE](https://github.com/s3huber/KeyboardX/blob/master/LICENSE).
