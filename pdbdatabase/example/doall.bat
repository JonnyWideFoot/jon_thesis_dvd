@echo off
del /Q .\dssp\*
cd chains
for %%p in (*.pdb) do ..\dofile.bat %%p
cd ..