#!/bin/sh
usr=manageusr
db=manage
echo "removing old $db db"
dropdb $db
dropuser $usr
createuser $usr
createdb $db
psql -q -c "alter user $usr with password '$usr'";
psql -q -c "alter database $db owner to $usr";
# use psql -e to echo sql along with errors while debugging
psql -d $db -q -c "GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO $usr;";
echo "postgres db $db and user $usr created"
