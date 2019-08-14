#!/bin/sh

sql="CREATE USER manage_courses_backend WITH SUPERUSER CREATEDB PASSWORD 'manage_courses_backend';"

# different variations of the call to psql that should work out of the box on vanilla installs of postgres

case "$(uname -s)" in # https://stackoverflow.com/questions/3466166/how-to-check-if-running-in-cygwin-mac-or-linux/27776822#27776822
   Darwin)
     psql -U postgres -c "$sql"
     ;;
   Linux)
     sudo -u postgres psql -c "$sql"
     ;;
  CYGWIN*|MINGW*|MSYS*)
     PGPASSWORD=postgres psql -U postgres -c "$sql"
     ;;
   *)
     echo 'unsupported OS'
     ;;
esac
