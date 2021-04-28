#!/bin/sh

############################################################################
#
# Script for mirroring PDB FTP archive using rsync
#
############################################################################

# This script is being provided to PDB users as a template for using rsync 
# to mirror the FTP archive from an anonymous rsync server. You may want 
# to review rsync documentation for options that better suit your needs.
#
# Author: Thomas Solomon
# Date:   November 1, 2002
# Updated by Kenneth J. Addess Jan 29,2004

# You should CHANGE THE NEXT THREE LINES to suit your local setup

MIRRORDIR=./dl/                 # your top level rsync directory
LOGFILE=./logs               # file for storing logs
RSYNC=rsync                             # location of local rsync

# You should NOT CHANGE THE NEXT TWO LINES

SERVER=rsync.rcsb.org                                # remote server name
PORT=33444                                           # port remote server is using

#
# Jon ....
#

${RSYNC} -rlpt -v -z --delete --port=$PORT $SERVER::ftp/ $MIRRORDIR > $LOGFILE 2>/dev/null

#
# Rsync the entire FTP archive /pub/pdb (Aproximately 19.4GB)
#

#${RSYNC} -rlpt -v -z --delete --port=$PORT $SERVER::ftp/ $MIRRORDIR > $LOGFILE 2>/dev/null


#
# Rsync only the data directory /pub/pdb/data (Aproximately 19 GB)
#

#${RSYNC} -rlpt -v -z --delete --port=$PORT $SERVER::ftp_data/ $MIRRORDIR/data > $LOGFILE 2>/dev/null


#
#  Rsync only the derived data directory /pub/pdb/derived_data (Aproximately 53 MB)
#

#${RSYNC} -rlpt -v -z --delete --port=$PORT $SERVER::ftp_derived/ $MIRRORDIR/derived_data > $LOGFILE 2>/dev/null


#
#  Rsync only the doc directory /pub/pdb/doc (Aproximately 200 MB)
#

#${RSYNC} -rlpt -v -z --delete --port=$PORT $SERVER::ftp_doc/ $MIRRORDIR/doc > $LOGFILE 2>/dev/null


#
#  Rsync only the software directory /pub/pdb/software (Aproximately 20 MB)
#

#${RSYNC} -rlpt -v -z --delete --port=$PORT $SERVER::ftp_software/ $MIRRORDIR/software > $LOGFILE 2>/dev/null


