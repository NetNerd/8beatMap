shift
FOR %%f IN (%*) DO (
hcaenc %%f "%%~dpnf.hca"
acbmaker "%%~dpnf.hca" "%%~dpnf.acb" -n "%%~nf"
del "%%~dpnf.hca"
)