for /r %%i in (*.jpg) do toktx --bcmp --linear --automipmap --lower_left_maps_to_s0t0 %%~dpni.ktx2 %%i