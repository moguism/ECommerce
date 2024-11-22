import { Order } from "./order";

export class User {
    id: number;
    name : string;
    email : string;
    password : string;
    address : string;
    role : string;
    orders: Order[];

    public constructor(id: number, name: string, email: string, password: string, address: string, role : string = "user") {
        this.id = id;
        this.name = name;
        this.email = email;
        this.password = password;
        this.address = address;
        this.role = role;
        this.orders = [];
    }
}
