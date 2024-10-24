export class User {

    name : string;
    email : string;
    password : string;
    address : string;
    role : string;

    public constructor(name: string, email: string, password: string, address: string, role : string = "user") {
        this.name = name;
        this.email = email;
        this.password = password;
        this.address = address;
        this.role = role;
    }
}
