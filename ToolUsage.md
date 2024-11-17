# Instructions

Please refer to the [Image types](https://github.com/Surihix/TxbImageTool/blob/master/ToolUsage.md#image-types) section, for more info about the image files.

## Extraction
### - Extract TXB_IMGB
This function extracts a image file from a txb (or txbh) and IMGB files.

The txb files (stored as .txbh in WPD archive containers) contain the DDS header data for the image as well as each mip's offset and size when stored in the IMGB file. the txb file is mainly found inside TRB files and there can be more than one txb file inside a TRB file. the paired IMGB file for the TRB file, would contain the image data for each txb file, that is packed inside the TRB file.

### - Extract XGR_IMGB
This function extracts one or more image files from a XGR and IMGB files

Apart from image files, an xgr file can also contain other types of files too and they will all be extracted.

## Conversion
### - Update existing TXB
This function allows you to quickly update an exisitng image file's data inside a IMGB file. you will have to select the txb file, the imgb file, and the folder where the image file is present.

The txb file will not be modified and will instead be used for getting the image filename and for locating the image file's offsets in the imgb file. do note that the image file that you are updating, should have similar dimensions, pixel format, and mip count as the original image file.

### - Create new TXB
This function allows you to create a TXB and IMGB file pair for a image file

For this function, you have to select the folder where the image file is present and then set the image type, and the gtex version. if the image type is cubemap or stack, then make sure the appropriate image files are present in the folder. 

Refer to the [Packing image type](https://github.com/Surihix/TxbImageTool/blob/master/ToolUsage.md#packing-image-type) section for more information.


### - Create XGR from folder
This function will create a XGR and IMGB file from a folder containing one or more image files.

The foldername will be used as the XGR and IMGB filenames. also refer to the [Packing image type](https://github.com/Surihix/TxbImageTool/blob/master/ToolUsage.md#packing-image-type) section for more information.

To use this function, a ``!!XGR_List.json`` must be present inside the folder containing the image files. this json file would contain information about each files present in the folder like the filename (without extension), the file extension, a boolean value that determines whether the file is a image, the GTEX_Version to use if the file is a image and the image type. 

Refer to the template given below and create your json file accordingly.

Here is a rough template that you can follow for your `!!XGR_List.json` file:
```json
{
    "totalFileCount": 5,
    "files" : [
        {
            "fileName": "c874F_a",
            "extension": "dds",
            "isImage": true,
            "gtexVersion": 3,
            "imageType": "classic"
        },
        {
            "fileName": "DrawEnv_proxy_0",
            "extension": "dds",
            "isImage": true,
            "gtexVersion": 3,
            "imageType": "cubemap"
        },
        {
            "fileName": "cm_sky_color1",
            "extension": "dds",
            "isImage": true,
            "gtexVersion": 3,
            "imageType": "stack"
        },
        {
            "fileName": "askcho",
            "extension": "bin",
            "isImage": false,
            "gtexVersion": 0,
            "imageType": ""
        },
        {
            "fileName": "gr_autosave",
            "extension": "ykd",
            "isImage": false,
            "gtexVersion": 0,
            "imageType": ""
        }
    ]
}
```

Set a proper value to the `gtexVersion` and `imageType` properties only if the file is a image. otherwise set it `0` and `""` respectively.

## Image types
This section will breifly go over the different types of image files that you would get from the TXB and IMGB files.

- If the image filenames end with `_cbmap_#`, then the image file is part of a cubemap set that uses a single header block file. the pixel format, dimensions, and mip counts are all shared by the images belonging to this set.

- If the image filenames end with `_stack_#`, then the image file is part of a stack set that uses a single header block file. the pixel format, dimensions, and mip count are all shared by the images belonging to this set. do note that the image files should contain only one mip and if there are multiple mips, then the image file itself will not be unpacked.

- If the image filenames do not contain `_cbmap_#` or `_stack_#` in their filenames, then its a classic type image.


## Packing image type
- If you are packing a `cubemap` type image, then make sure the folder contains six image files with similar dimensions, pixel format, mipcount, and with a identical starting filename. the difference in the files will be in the ending part of their filenames where each of them should end with `_cbmap_1`, `_cbmap_2`, `_cbmap_3`, `_cbmap_4`, `_cbmap_5`, and `_cbmap_6`.
  - For example let's say the image file that we are packing as a cubemap is called `DrawEnv_proxy_0`. then there should be six image files with the following names:
    - `DrawEnv_proxy_0_cbmap_1.dds`
    - `DrawEnv_proxy_0_cbmap_2.dds`
    - `DrawEnv_proxy_0_cbmap_3.dds`
    - `DrawEnv_proxy_0_cbmap_4.dds`
    - `DrawEnv_proxy_0_cbmap_5.dds`
    - `DrawEnv_proxy_0_cbmap_6.dds`

- If you are packing a `stack` type image, then make sure the folder contains image files with similar dimensions, pixel format, mipcount, and with a identical starting filename. the difference in the files will be in the ending part of their filenames where each of them should end with `_stack_1`, `_stack_2`, `_stack_3`, `_stack_4`,....etc
  - For example let's say the image file that we are packing as a stack is called `cm_sky_color1` and we want to have four of them. then there should be four image files with the following names:
    - `cm_sky_color1_stack_1.dds`
    - `cm_sky_color1_stack_2.dds`
    - `cm_sky_color1_stack_3.dds`
    - `cm_sky_color1_stack_4.dds`
