pipeline {
  options() {
    disableConcurrentBuilds()
  }
  agent {
    docker {
      image 'microsoft/dotnet:sdk'
      args '-v $HOME/.dotnet:/root/.dotnet -v $HOME/.nuget:/root/.nuget -u root'
    }
  }
  environment {
    NUGET_ACCESS_KEY = credentials('Nexus_Nuget_API_Key')
    NUGET_PUSH_REPO = 'http://ksjenkins.usrv.ubergenkom.no:8082/repository/nuget-hosted/'
  }
  stages {
    stage('Build') {
      steps {
        sh 'dotnet restore'
        sh 'dotnet build --no-restore -c Release'
      }
    }
    stage('Run tests') {
      steps {
        sh 'dotnet test --no-build --verbosity normal --logger "trx;LogFileName=results.trx"'
      }
      post {
        success {
          xunit(  thresholds: [ skipped(failureThreshold: '0'), failed(failureThreshold: '0') ],
                  tools: [ MSTest(pattern: '**/*.trx') ]
          )
        }
      }
    }
    stage('Push to NuGet server') {
      steps {
          sh 'dotnet nuget push */bin/Release/*.nupkg -k $NUGET_ACCESS_KEY -s $NUGET_PUSH_REPO'
      }
    }
  }

}
