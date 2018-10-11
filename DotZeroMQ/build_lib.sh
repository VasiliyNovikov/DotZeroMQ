#!/bin/bash
set -e

wget https://github.com/zeromq/libzmq/releases/download/v4.2.5/zeromq-4.2.5.tar.gz
tar xf zeromq-4.2.5.tar.gz -C ./
mv zeromq-4.2.5 zeromq
cd zeromq/
./configure
make
