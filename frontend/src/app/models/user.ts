export interface User {

    name : string,
    email : string,
    password : string,
    address : string,
    role : string

    private constructor(success: boolean, statusCode: number, error: string = null, data: T = null) {
        this.success = success;
        this.error = error;
        this.statusCode = statusCode;
        this.data = data;
      }
}
