@echo off

echo universe = vanilla > templateTest.que
echo should_transfer_files = YES >> templateTest.que
echo WhenToTransferOutput = ON_EXIT_OR_EVICT >> templateTest.que
echo "REQUIREMENTS = (OpSys == "WINNT51") && (KFlops == 300000) && (Arch == "INTEL")" >> templateTest.que
echo Executable = loopbuilder.exe >> templateTest.que
echo Error = logerr >> templateTest.que
echo Output = logout >> templateTest.que
echo Log = loglog >> templateTest.que

for %%p in (../filtered2/*.pdb) do dofile.bat %%p