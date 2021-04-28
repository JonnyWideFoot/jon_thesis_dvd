@echo off

echo Doing file : %1 : redirecting to %1.tra
makepsf -infile makepsf.conf -pdbfile ../filtered2/%1 -filestem %1

echo transfer_input_files=amber03ua.ff,dictallHnew.pdb,whole_minim.conf,Optimal_Loop_6-5-8.angleset,%1.tra >> templateTest.que
echo arguments = -infile whole_minim.conf -inputfile %1.tra -stem %1_Min >> templateTest.que
echo queue >> templateTest.que