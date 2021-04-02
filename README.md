# VVV Rippers

This project contains three sub projects:

* In the `BaseNoesis` you will find the original Noesis script which has been edited by me to support VVVTune models. This one is not capable of ripping bone hierarchy or face expressions. The script can be used like a Noesis plugin.
* In the `OrochiPMX` folder you'll find a Visual Studio 2019 .NET 4.5 project written in C# which can convert Orochi4 models from VVVTune to PMX - this one will perform a flat conversion - it will not adjust the PMX file for use in MikuMikuDance. It will do so by scanning entire directory trees but the actual conversion class is in GsmdlFile.cs and easy to use. This one can rip hierarchy and face expressions.
* In the `VVVPMX` there is a project which works through PMX files created by `OrochiPMX` and adjusts them for proper use with MikuMikuDance (Japanese names, A pose, bone order, center bone, IK and such).


## BaseNoesis

Copy `fmt_Orochi4_PC.py` into your Noesis import folder and you should be able to use it.

## OrochiPMX

### Using it in its current form 

In its current form it can be used to convert all kinds of character models. It expects the models to reside in directories called `GxArchivedFile[Number].csh_extract` which is the default name when you use chrrox Orochi4 unpacker for Gunslinger Stratos available at [his post on the XeNTaX forums (https://forum.xentax.com/viewtopic.php?t=12687)](https://forum.xentax.com/viewtopic.php?t=12687) . Within it it will search for mdc model data (`*.C457B87E` files) which contains the string `model:MODEL/CHARA/` - it will then assume this is the MDC data of a model and convert it to PMX.

### Using the conversion class

This is easy. You just need the following code for conversion.
```cs
GsmdlFile gsmdlFile = new GsmdlFile(
    mdlFile, // path to the MDL file
    mdcFilePath, //this is the path to the mdc data / C457B87E file - Can be null
    pmxOutputPath //PMX file to creat
);

gsmdlFile.loadMdc(); //Only if mdcFilePath isn't null
gsmdlFile.loadMdlHeader(); //Load and convert model
```

This works with rigged and static models.

### VVVPMX

This is a rather special project. It uses PMX files converted by `OrochiPMX` and arranges them for MikuMikuDance. It doesn't really do anything to a model other than renaming bones, rotating them, renaming materials, ordering textures, arranging face expressions and a lot of other stuff for easy automation of an otherwise extremely tedious process. Unless you want to use the models made with `OrochiPMX` in MikuMikuDance you will not need this.

# Blablabla, copyright

This is based on work by chrrox and he shall earn 99% of the credits for it. I release it under Apache 2.0 license which is a lot of legal text for "Feel free to use for whatever, credit is appreciated but not necessary, don't sue me!"