# ArgosSharp

ArgosSharp is a backend-oriented job execution platform designed to practice clean architecture, asynchronous processing, cloud-compatible infrastructure, and infrastructure as code.

## Architecture

```text
Client
  |
  v
ASP.NET Core API
  |
  +--> PostgreSQL
  |
  +--> JobExecution Queue
          |
          v
        SQS
          |
          v
      JobWorker
          |
          v
  ProcessorOrchestrator
      |
      +--> Scraper
      +--> ResultExport
      +--> ArtifactService
                |
                v
             S3
```

The local environment is AWS-like:

```text
Podman Compose
    |
    +--> PostgreSQL
    |
    +--> Floci
           |
           +--> SQS
           +--> S3
```

## Infrastructure, Cloud Resources, and Application Usage

A key architectural distinction in ArgosSharp is separating three responsibilities.

### 1. Compose: what infrastructure exists?

Podman Compose answers:

> What infrastructure exists?

It starts the required containers, configures ports, environment variables, volumes, and networking.

For example:

```yaml
services:
  postgres:
    image: postgres:17

  floci:
    image: floci/floci:latest
```

Compose provides the environment. It does not define individual SQS queues or S3 buckets.

### 2. Terraform: what cloud resources does it contain?

Terraform answers:

> What cloud resources should exist inside that infrastructure?

For example:

```hcl
resource "aws_sqs_queue" "job_executions" {
  name = "argos-job-executions"
}
```

The Terraform provider remains `hashicorp/aws`. Its endpoints are redirected to Floci during local development.

```text
Terraform
   |
   | hashicorp/aws
   v
Floci :4566
   |
   +--> SQS
   +--> S3
```

The resources remain AWS resources such as `aws_sqs_queue` and `aws_s3_bucket`.

### 3. Application: how are the resources used?

The .NET application accesses the resources through AWS SDKs and application abstractions.

```text
IJobExecutionQueue
        |
        v
SqsJobExecutionQueue
        |
        v
AWS SDK for .NET
        |
        v
SQS
```

Likewise:

```text
IArtifactStorage
        |
        v
S3ArtifactStorage
        |
        v
AWS SDK for .NET
        |
        v
S3
```

The application should not need to know whether those resources are backed by Floci locally or AWS in production.

## Why Floci?

Floci is the local AWS-compatible environment used for development.

The learning goal is **AWS**, not Floci itself. The project is intended to practice:

- Amazon SQS
- Amazon S3
- AWS SDK for .NET
- Terraform
- Dead-letter queues
- Visibility timeouts
- Retry behavior
- Infrastructure as Code
- Cloud-compatible application design

Locally:

```text
AWS SDK
   |
   v
Floci
   |
   +--> SQS
   +--> S3
```

In production, the same application architecture can target:

```text
AWS SDK
   |
   v
AWS
   |
   +--> SQS
   +--> S3
```

## MinIO vs S3

MinIO is an S3-compatible object-storage implementation. It is useful when the goal is specifically to provide local S3-compatible storage.

Amazon S3 is the managed AWS service.

The application should depend on an abstraction such as:

```text
IArtifactStorage
```

rather than directly depending on MinIO or Floci.

This makes it possible to use a local implementation during development and AWS S3 in production without changing application logic.

## LocalStack vs Floci

LocalStack and Floci provide similar AWS-compatible local environments.

ArgosSharp uses Floci because it provides the AWS-like services needed for the project without the authentication workflow that caused problems during the LocalStack setup.

The important architectural decision is to keep the application dependent on AWS APIs and abstractions rather than on the emulator itself.

## Terraform Workflow

Start the infrastructure:

```bash
podman compose up -d
```

Then provision cloud resources:

```bash
cd infrastructure/terraform

terraform init
terraform plan
terraform apply
```

The resulting model is:

```text
Podman Compose
      |
      v
   Floci
      |
      v
Terraform
      |
      +--> SQS Queue
      +--> SQS DLQ
      +--> S3 Bucket
```

## Job Execution Flow

```text
CreateJobExecution
        |
        v
Persist JobExecution
        |
        v
Enqueue execution
        |
        v
SQS
        |
        v
JobWorker
        |
        v
ProcessorOrchestrator
        |
        +--> Scraper
        +--> ResultExport
        +--> ArtifactService
                    |
                    v
                   S3
```

On successful processing, the worker acknowledges/deletes the SQS message.

On failure, the message remains available for retry. After the configured receive limit is exceeded, SQS moves it to the dead-letter queue.

## Design Goal

ArgosSharp deliberately separates:

- Domain concepts
- Application use cases
- Infrastructure implementations
- Cloud resource provisioning
- Local infrastructure
- External service integrations

The goal is to make the local architecture resemble a production cloud architecture closely enough that moving from local infrastructure to AWS is primarily a deployment/configuration concern.
