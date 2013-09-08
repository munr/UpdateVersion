@if "%VSINSTALLDIR%"=="" goto Usage

nmake

@pause
@goto end

:Usage

@echo. You must first run vsvars32.bat before you can run this example.
@echo.
@pause

@goto end

:end