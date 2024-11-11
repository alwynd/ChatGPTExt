# ChatGPT API Command-Line Tool

## Overview
This command-line tool interacts with OpenAI's ChatGPT API, allowing users to perform actions like listing available models and submitting code review/edit requests.

### Features:
- **List Available Models**: Fetches and displays a list of available GPT models.
- **Request Code Review/Edit**: Sends a request with specified text or file content for ChatGPT to review/edit code.

## Usage
The tool supports various command-line options to configure API settings and specify requests. 

### Command-Line Options:
- **API Key and Project Configuration**:
  - `-api-key APIKEY`: Sets the OpenAI API key (required).
  - `-org-id ORGANIZATIONID`: Sets the OpenAI organization ID (required if using a project-specific API key).
  - `-project-id PROJECTID`: Sets the OpenAI project ID (required if using a project-specific API key).

- **Actions**:
  - `-list-models`: Lists currently available GPT models.
  - `-request TEXT`: Specifies a request file for code review/edit. Text in this file is sent to ChatGPT for processing.

- **Optional Parameters for Requests**:
  - `-model gptmodel`: Sets the specific GPT model to use (defaults to a general-purpose GPT model if unspecified).
  - `-system SYSTEMMESSAGE`: Sets an optional system message to configure the model’s response context (e.g., as a code assistant/reviewer).

### Examples
1. **Listing Models**:
   ```sh
   appname -api-key YOUR_API_KEY -list-models
   ```
This command lists all currently available GPT models.

2. **Requesting a Code Review/Edit:**:
   ```sh
   appname -api-key YOUR_API_KEY -request requestfile.txt -model gpt-4 -system "Code assistant"
   ```
This command sends a code review/edit request based on requestfile.txt and uses gpt-4 as the model with a system message of "Code assistant".


## Additional Details
- **Clipboard Integration**: If there is text in the clipboard, it is assumed to be code and will be appended to the request from the file.
- **Debugging Information**: Logs relevant details, including projectId, organizationId, and apiKey.

## Prerequisites
- .NET environment configured to run async tasks.
- Valid OpenAI API key and, if applicable, organization and project IDs.

## License
- This project is licensed under the MIT License.