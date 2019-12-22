@echo off

set BRANCHES=asaf dalik yael

for %%b in (%BRANCHES%) do ( 
   git checkout %%b
   git pull
   git rebase master
   git push
)
echo Done
