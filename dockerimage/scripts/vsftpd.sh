#!/bin/bash

if [ ! -z "$FTPServerIp" ]; then
echo "Starting ftp server"

[ -f /etc/vsftpd.conf ] || cat <<EOF > /etc/vsftpd.conf
listen=YES
anonymous_enable=YES
dirmessage_enable=YES
use_localtime=YES
connect_from_port_20=YES
secure_chroot_dir=/var/run/vsftpd/empty
write_enable=NO
seccomp_sandbox=NO
xferlog_std_format=NO
log_ftp_protocol=YES
syslog_enable=YES
hide_ids=YES
seccomp_sandbox=NO
pasv_enable=YES
port_enable=YES
anon_root=/var/ftp/public
pasv_min_port=50000
pasv_max_port=50100
ftpd_banner=SecureBank
EOF

/usr/sbin/vsftpd "$@"
fi