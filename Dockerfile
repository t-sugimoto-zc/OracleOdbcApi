FROM ubuntu:20.04

# 必要なリポジトリの追加
RUN echo "deb http://archive.ubuntu.com/ubuntu focal main universe" > /etc/apt/sources.list

# 必要なパッケージのインストール
RUN apt-get update

RUN apt-get install -y libaio1
RUN apt-get install -y wget
RUN apt-get install -y unzip
RUN apt-get install -y unixodbc

# Oracle Instant Clientのインストール
COPY instantclient-basic-linux.x64-12.2.0.1.0.zip /opt/oracle/
COPY instantclient-odbc-linux.x64-12.2.0.1.0-2.zip /opt/oracle/
RUN cd /opt/oracle
RUN unzip /opt/oracle/instantclient-basic-linux.x64-12.2.0.1.0.zip -d /opt/oracle
RUN unzip /opt/oracle/instantclient-odbc-linux.x64-12.2.0.1.0-2.zip -d /opt/oracle
RUN rm -f /opt/oracle/*.zip
RUN ln -s /opt/oracle/instantclient_12_2/libclntsh.so.12.1 /opt/oracle/instantclient_12_2/libclntsh.so
RUN ln -s /opt/oracle/instantclient_12_2/libocci.so.12.1 /opt/oracle/instantclient_12_2/libocci.so
RUN ln -s /opt/oracle/instantclient_12_2 /opt/oracle/instantclient
RUN ls -a /opt/oracle/instantclient
RUN echo /opt/oracle/instantclient > /etc/ld.so.conf.d/oracle-instantclient.conf
RUN ldconfig

# TNS_ADMIN ディレクトリの作成と環境変数の設定
RUN mkdir -p /opt/oracle/instantclient/network/admin
ENV TNS_ADMIN=/opt/oracle/instantclient/network/admin

# ユーザデータソース用ディレクトリの作成と環境変数の設定
RUN mkdir -p /home/ubuntu
ENV HOME=/home/ubuntu

ENV ODBCINI=/etc/odbc.ini

ENV LD_LIBRARY_PATH=/opt/oracle/instantclient:$LD_LIBRARY_PATH

# iniファイルとoraファイルの作成
COPY odbc.ini /etc/
COPY .odbc.ini /home/ubuntu/
COPY odbcinst.ini /etc/
COPY tnsnames.ora /opt/oracle/instantclient/network/admin/

# ドライバーの登録
RUN odbcinst -i -d -f /etc/odbcinst.ini
RUN odbcinst -q -d

# コンテナを1時間起動させる
CMD ["sleep", "3600"]
