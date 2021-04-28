Modified Slightly from WinCount 1.0
(C) AderSoftware 2003
http://www.adersoftware.com
http://www.codeproject.com/csharp/WinCount.asp

You can use this software for no charge, but you cannot 
redistribute it without author's permission.

Installation:
Just put the WinCount.exe file somewhere on your harddrive and
run the program by double clicking the executable.

To install in VS.NET as external tool:
1. Go to Tools/External Tools
2. Click On Add Button
3. Fill the fields as follows:
	Name: WinCount
	Command: Path to the winCount.exe file
	Arguments: $(SolutionDir)
	Initial Directory: blank (nothing at all)
4. Click OK
5. Now you can run the program by going to tools and select wincount