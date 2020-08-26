# RenderDemo

A command line tool that renders out TF2 demos using SourceDemoRender.

```
D:\Me\Desktop\RenderDemo>renderdemo -exepath "G:\Steam Library\Team Fortress 2\hl2.exe" -demo "demos\stv demo" -start 1500 -end max -out myClip -cmd "spec_player Me; spec_mode 5"

Demo: G:\Steam Library\Team Fortress 2\tf\demos\stv demo.dem
Output: D:\Me\Desktop\RenderDemo\myClip.avi
Start tick: 1500 | End tick: 8497 | Profile: both
Commands: "spec_player Me; spec_mode 5"

Starting SDR laucher...  TF2 has started, closed SDR launcher.
TF2 loaded with profile 'both'. Demo is loading.
Pass 1/2: Recording Video.
Recording progress |>>>>>>>>>>>>>>>>>>>>>>>>>| 100%
Pass 2/2: Recording Audio.
Recording progress |>>>>>>>>>>>>>>>>>>>>>>>>>| 100%
Recording finished successfully.
```


- Not compatible with Lawena, uses your current config. Shouldn't change any settings.
- Recording happens in the background, you still use your PC (with CPU usage at 100%).
- Uses SDR's MultiProcess extension - multiple game instances can be ran at the same time.
- You should know how to use Windows' **cmd.exe** and batch files.
- For more info on how to use it, try `renderdemo -help`.


## What it does

1. Creates a VDM file to control the recording
2. Starts TF2 with SDR
3. Loads the demo, fast forwards to start tick
4. When recording is finished, kills TF2 and returns

## SDR Notice

SDR version 32.2 patched for TF2 is required. It used to be avialible [here](https://github.com/laurirasanen/SourceDemoRender/releases/tag/32.2-a1). But a newer version that no longer requires the patch came out, so the author took it down. The new version however doesn't support audio recording (plus I'm too lazy to update this for it), so I've included the right SDR version in the download.

All credits to [crashfort](https://github.com/crashfort) and [laurirasanen](https://github.com/laurirasanen).

## Exaples

### STV demos

Use the `-cmd` argument to specify TF2 commands to be executed before the recording starts.  
`spec_player playername` - spectate player by name (doesn't have to be the whole name)  
`spec_mode 5` - first person

E.g. `-cmd "spec_player Me; spec_mode 5"`.

### Look through demos and render at the same time

1. Start TF2 using SDR launcher, load your demo, watch it, find your start and end ticks.
2. Tab out, start RenderDemo. While it's recording, tab back into TF2, load the next demo, find ticks, etc.
3. When RenderDemo is finished, start the next render. And so on.

### Queue renders

You can queue multiple renders by using a batch file like this:

```
renderdemo -exepath "G:\Steam Library\Team Fortress 2\hl2.exe" -demo demo1 -start 1900 -end 3200 -out clip1.avi
renderdemo -exepath "G:\Steam Library\Team Fortress 2\hl2.exe" -demo demo2 -start 2440 -end max -out clip2.avi
renderdemo -exepath "G:\Steam Library\Team Fortress 2\hl2.exe" -demo demo3 -start 4720 -end 6000 -out clip3.avi
pause
```

Running this will render out those three clips one after another. If TF2 crashes during one of them, that clip is skipped and queue continues.

## Notes

- During audio pass game volume is at maximum, so if you focus the window, it will be LOUD.
- Framerate is controlled by `sdr_video_fps`, default is 60. Set it using the `-cmd` argument: `-cmd "sdr_video_fps 120"`
- Read about more SDR options [here](https://github.com/crashfort/SourceDemoRender/blob/32.1/ReadMe.md).
