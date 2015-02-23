While developing there may be more than one schema file. Schema files with comments use the 
following pattern _keyboard*-commented.xsd_. The corresponding _keyboard*.xsd_ file is without 
comments (i.g. it is "minified").

For reading a schema file **always** use the **commented** form. As the name suggests, it contains 
a lot of useful additional information in form of comments.

To minify a schema file with comments the shell script *minify.sh* could be used. 
An alternative is to replace XML comments via the following regex pattern *<\!--(.|\n)\*?-->* 
inside Visual Studio via the replace dialog (CTRL + SHIFT + H).

### dev1

Current schema used for development.

### keyboard-20150220

Changes:

 * increased minimal version to 20150101
 * restrict rotation attribute of icon by enumeration

### keyboard-20141222

Changes:

 * redefined default section
 * use keys instead of ids
 * grid can now define drawer
 * added massive style
 * modeled action types

### keyboard-20141218

Changes:

 * modeled scanner defaults for keyboard and grid
 * added attribute for default grid
 * modeled TCP destination
 * added TCP action
 * modeled action types

### keyboard-20140828

First reasonable version of schema, but a lot of stuff is still missing.
