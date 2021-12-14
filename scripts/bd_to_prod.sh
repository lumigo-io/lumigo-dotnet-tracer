#!/usr/bin/env bash
set -Eeo pipefail

setup_git() {
    git config --global user.email "no-reply@build.com"
    git config --global user.name "CircleCI"
    git checkout master
    # Avoid version failure
    git stash
}

push_tags() {
    git push origin master --tags
}

echo ".____                  .__                  .__        ";
echo "|    |    __ __  _____ |__| ____   ____     |__| ____  ";
echo "|    |   |  |  \/     \|  |/ ___\ /  _ \    |  |/  _ \ ";
echo "|    |___|  |  /  Y Y  \  / /_/  >  <_> )   |  (  <_> )";
echo "|_______ \____/|__|_|  /__\___  / \____/ /\ |__|\____/ ";
echo "        \/           \/  /_____/         \/            ";
echo
echo "Deploy .Net Tracer"

setup_git

echo "Getting latest changes from git"
changes=$(git log $(git describe --tags --abbrev=0)..HEAD --oneline)

apt update
echo
echo "*** Installing Nuget... ***"
echo
curl -o /usr/local/bin/nuget.exe https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
echo
echo "*** Installing mono ***"
echo
apt install -y apt-transport-https dirmngr gnupg ca-certificates
apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb https://download.mono-project.com/repo/debian stable-buster main" | tee /etc/apt/sources.list.d/mono-official-stable.list
apt update
apt install -y mono-devel

echo
echo "*** Installing pip ***"
echo
apt install -y python3-pip
pip3 install --upgrade bumpversion
bumpversion patch --message "{current_version} → {new_version}. Changes: ${changes}"

echo
echo "*** Uploading to Nuget ***"
echo "Building..."
dotnet build -c Release src/Lumigo.DotNET/Lumigo.DotNET.csproj
echo "Packing..."
mono /usr/local/bin/nuget.exe pack src/Lumigo.DotNET/ -Prop Configuration=Release -IncludeReferencedProjects
echo "Pushing..."
mono /usr/local/bin/nuget.exe push *.nupkg -Source https://api.nuget.org/v3/index.json -ApiKey ${NUGET_KEY} -NonInteractive
push_tags
echo \{\"type\":\"Release\",\"repo\":\"${CIRCLE_PROJECT_REPONAME}\",\"buildUrl\":\"${CIRCLE_BUILD_URL}\"\} | curl -X POST "https://listener.logz.io:8071?token=${LOGZ}" -v --data-binary @-
echo "Done"