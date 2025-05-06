export interface IResultBaseDto<T>  {
  status: boolean;
  message?: string[];
  data?: T;
}

export interface Device {
  id: string;
  identifier: string;
  description: string;
  manufacturer: string;
  url: string;
  commands?: Commands[];
}
export interface Commands {
  operation: string
  description: string
  result: string
  format: string,
  command: Command
}

export interface Command {
  commandName: string
  parameters: Parameter[]
}

export interface Parameter {
  name: string
  description: string
}


