# ColorTabs2019

Color Tabs Extension for Visual Studio 2019 (not earlier 16.11). Hopefully this version of VS will not be changed dramatically.


![Example](example0.png)


This Vsix will add a colored rectangle left of your tab name. Primary color is based on your project file path. Secondary color is based on folder of your file. No way to determine specific color for specific project exsist yet.

Colored rectangles will be visible only if you are using a left/right layout for the tab panel. Top layout does not support yet.

You can change a foreground for tab names in Tools\Options\Color Tabs 2019 to bring it to accordance of your VS theme. Also you can disable secondary color.


I DO NOT test it against:

- any Visual Studio theme except dark
- any project types except `csproj`

**Side effect: RMB will NOT work on modified tabs titles!** Sadly to say, I don't understand how VS processed right clicks in that panel...

Please keep in mind: this Vsix make a hardcore surgery on VS GUI, so side effects can occurs. In that case you can disable vsix in Tools\Options\Color Tabs 2019, then choose tabl layout `Top`, and the swith the layout back to your preferenced position (left, right). No need to restart VS.
