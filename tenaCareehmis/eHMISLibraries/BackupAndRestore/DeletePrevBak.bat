@echo off

@echo off

set yyyy=

set $tok=1-3
for /f "tokens=1 delims=.:/-, " %%u in ('date /t') do set $d1=%%u
if "%$d1:~0,1%" GTR "9" set $tok=2-4
for /f "tokens=%$tok% delims=.:/-, " %%u in ('date /t') do (
 for /f "skip=1 tokens=2-4 delims=/-,()." %%x in ('echo.^|date') do (
    set %%x=%%u
    set %%y=%%v
    set %%z=%%w
    set $d1=
    set $tok=))

if "%yyyy%"=="" set yyyy=%yy%
if /I %yyyy% LSS 100 set /A yyyy=2000 + 1%yyyy% - 100

set CurDate=%mm%/%dd%/%yyyy%

set dayCnt=%2

if "%dayCnt%"=="" set dayCnt=2

REM Substract your days here
set /A dd=1%dd% - 100 - %dayCnt%
set /A mm=1%mm% - 100

:CHKDAY

if /I %dd% GTR 0 goto DONE

set /A mm=%mm% - 1

if /I %mm% GTR 0 goto ADJUSTDAY

set /A mm=12
set /A yyyy=%yyyy% - 1

:ADJUSTDAY

if %mm%==1 goto SET31
if %mm%==2 goto LEAPCHK
if %mm%==3 goto SET31
if %mm%==4 goto SET30
if %mm%==5 goto SET31
if %mm%==6 goto SET30
if %mm%==7 goto SET31
if %mm%==8 goto SET31
if %mm%==9 goto SET30
if %mm%==10 goto SET31
if %mm%==11 goto SET30
REM ** Month 12 falls through

:SET31

set /A dd=31 + %dd%

goto CHKDAY

:SET30

set /A dd=30 + %dd%

goto CHKDAY

:LEAPCHK

set /A tt=%yyyy% %% 4

if not %tt%==0 goto SET28

set /A tt=%yyyy% %% 100

if not %tt%==0 goto SET29

set /A tt=%yyyy% %% 400

if %tt%==0 goto SET29

:SET28

set /A dd=28 + %dd%

goto CHKDAY

:SET29

set /A dd=29 + %dd%

goto CHKDAY

:DONE

if /I %mm% LSS 10 set mm=0%mm%
if /I %dd% LSS 10 set dd=0%dd%



for /f "tokens=1-4 delims=/ " %%a in ('Date /t') do (
set Month=%%b
set Day=%%c
set Year=%%d
)
for /f "tokens=1-4 delims=: " %%a in ('time /t') do (
set Hour=%%a
set min=%%b

)
    if "%mm%" == "01" set currentMonth=January
    if "%mm%" == "02" set currentMonth=February
    if "%mm%" == "03" set currentMonth=March
    if "%mm%" == "04" set currentMonth=April
    if "%mm%" == "05" set currentMonth=May
    if "%mm%" == "06" set currentMonth=June
    if "%mm%" == "07" set currentMonth=July
    if "%mm%" == "08" set currentMonth=August
    if "%mm%" == "09" set currentMonth=September
    if "%mm%" == "10" set currentMonth=October
    if "%mm%" == "11" set currentMonth=November
    if "%mm%" == "12" set currentMonth=December

    if "%dd%" == "01" set dd=1
    if "%dd%" == "02" set dd=2
    if "%dd%" == "03" set dd=3
    if "%dd%" == "04" set dd=4
    if "%dd%" == "05" set dd=5
    if "%dd%" == "06" set dd=6
    if "%dd%" == "07" set dd=7
    if "%dd%" == "08" set dd=8
    if "%dd%" == "09" set dd=9

    if "%Hour%" == "01" set Hour=1
    if "%Hour%" == "02" set Hour=2
    if "%Hour%" == "03" set Hour=3
    if "%Hour%" == "04" set Hour=4
    if "%Hour%" == "05" set Hour=5
    if "%Hour%" == "06" set Hour=6
    if "%Hour%" == "07" set Hour=7
    if "%Hour%" == "08" set Hour=8
    if "%Hour%" == "09" set Hour=9

    if "%min%" == "01" set Hour=1
    if "%min%" == "02" set Hour=2
    if "%min%" == "03" set Hour=3
    if "%min%" == "04" set Hour=4
    if "%min%" == "05" set Hour=5
    if "%min%" == "06" set Hour=6
    if "%min%" == "07" set Hour=7
    if "%min%" == "08" set Hour=8
    if "%min%" == "09" set Hour=9

set FileName=eHMIS_%currentMonth%_%dd%_%yyyy%_%Hour%_%min%.bak
CD HMISBackUpDB
IF EXIST %FileName%. (del %FileName%.)