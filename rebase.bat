@echo off

set BRANCHES=dalik yael asaf

for %%b in (%BRANCHES%) do ( 
   git checkout %%b
   git pull
   git rebase master
   git push
)
echo Done
