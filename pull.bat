@echo off

set BRANCHES=dalik yael master

for %%b in (%BRANCHES%) do ( 
   git checkout %%b
   git pull
)
echo Done
