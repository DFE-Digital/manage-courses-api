#!/bin/sh
if [ $# -lt 3 ]
then
  echo "Params: username, password, database"
  exit 1
fi
usr=$1
pass=$2
db=$3
echo "removing old $db db"
dropdb $db
dropuser $usr
createuser $usr
createdb $db
psql -q -c "alter user $usr with password '$pass'";
psql -q -c "alter database $db owner to $usr";
# use psql -e to echo sql along with errors while debugging
psql -d $db -q -c "GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO $usr;";
echo "postgres db $db and user $usr created"
