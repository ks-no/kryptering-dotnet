pipeline {
  options() {
    disableConcurrentBuilds()
  }
  agent {
    docker {
      image 'microsoft/dotnet:sdk'
      args '-v ${PWD}:/app -w /app'
    }
  }
  stages {
    stage('Build') {
      steps {
        sh 'dotnet restore'
        sh 'dotnet dotnet build --no-restore -c Release'
      }
    }
    stage('Run tests') {
      steps {
        sh 'dotnet test'
      }
    }
  }
  post {
    always {
      xuint(thresholds: [ skipped(failureThreshold: '0'), failed(failureThreshold: '0') ])
    }
  }
}
