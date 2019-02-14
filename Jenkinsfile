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
  stages {
    stage('Build') {
      steps {
        sh 'dotnet restore'
        sh 'dotnet build --no-restore -c Release'
      }
    }
    stage('Run tests') {
      steps {
        sh 'dotnet test --verbosity normal --logger "trx;LogFileName=results.trx"'
      }
    }
  }
  post {
    success {
      xunit(  thresholds: [ skipped(failureThreshold: '0'), failed(failureThreshold: '0') ],
              tools: [ MSTest(pattern: '**/*.trx') ]
      )
    }
  }
}
