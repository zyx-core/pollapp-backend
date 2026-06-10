pipeline {
    agent any

    environment {
        DOCKER_IMAGE = 'zyx-core/pollapp-backend'
        DOCKER_TAG = "v${env.BUILD_NUMBER}"
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }
        
        stage('Restore Dependencies') {
            steps {
                echo 'Restoring .NET packages...'
                bat 'dotnet restore "./PollApp.Backend.csproj"'
            }
        }

        stage('Build') {
            steps {
                echo 'Building backend...'
                bat 'dotnet build "./PollApp.Backend.csproj" -c Release --no-restore'
            }
        }

        stage('Docker Build') {
            steps {
                echo 'Building Docker Image...'
                bat "docker build -t ${env.DOCKER_IMAGE}:${env.DOCKER_TAG} -t ${env.DOCKER_IMAGE}:latest ."
            }
        }

        /* 
        // Optional: Push to registry
        stage('Docker Push') {
            steps {
                withCredentials([usernamePassword(credentialsId: 'dockerhub-credentials', usernameVariable: 'DOCKER_USER', passwordVariable: 'DOCKER_PASS')]) {
                    sh "echo \$DOCKER_PASS | docker login -u \$DOCKER_USER --password-stdin"
                    sh "docker push ${env.DOCKER_IMAGE}:${env.DOCKER_TAG}"
                    sh "docker push ${env.DOCKER_IMAGE}:latest"
                }
            }
        }
        */
    }

    post {
        always {
            echo 'Pipeline finished.'
        }
        success {
            echo 'Build successful!'
        }
        failure {
            echo 'Build failed. Check the logs.'
        }
    }
}
