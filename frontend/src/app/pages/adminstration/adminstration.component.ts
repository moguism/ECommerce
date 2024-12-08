import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { User } from '../../models/user';
import { UserService } from '../../services/user.service';
import { ProductService } from '../../services/product.service';
import { FormsModule } from '@angular/forms';
import { EurosToCentsPipe } from '../../pipes/euros-to-cents.pipe';
import { CommonModule, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';

// Pipe Import
import { CorrectDatePipe } from '../../pipes/correct-date.pipe';
import { Product } from '../../models/product';
import { Order } from '../../models/order';
import { ProductsToBuy } from '../../models/products-to-buy';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { Category } from '../../models/category';
import { ProductToInsert } from '../../models/product-to-insert';
import { Router } from '@angular/router';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-adminstration',
  standalone: true,
  imports: [HeaderComponent, FormsModule, CorrectDatePipe, EurosToCentsPipe, TranslatePipe,CommonModule,RouterLink],
  templateUrl: './adminstration.component.html',
  styleUrl: './adminstration.component.css',
  providers:[DecimalPipe]
})
export class AdminstrationComponent {
  constructor(private userService: UserService, private productService: ProductService,private decimalPipe:DecimalPipe, private router: Router, private api : ApiService) {
  }


  user: User | null = null;
  orders: Order[] = [];
  elementShowing: string = "";
  allUsers: User[] = [];
  allProducts: Product[] = [];
  formState: string | null = null; // Puede ser 'editRole' o 'createProduct'
  editRoleValue: string = "";
  newProductName: string = "";
  newProductPrice: number = 0;
  newProductCategory:string="";
  newProductStock: number = 0;
  newproductDescription: string="";
  selectedUser: User | null = null;
  Product:Product|null=null;
  category:string="";
  categorytranslate:string="";
  image: File | null = null
  pricedecimal:string="";
  create : boolean = false;
  idToUpdate : number = 0

  async ngOnInit(): Promise<void> {
    /*if(this.api.jwt == null || this.api.jwt == "")
    {
      this.router.navigateByUrl("login")
    }*/
    await this.getUser();
    if(this.user?.role!="Admin"){
      this.router.navigate(['not-found']);
    }
  }

  async getUser() {
    const result = await this.userService.getUser();
    if (result) {
      this.user = result;
      this.changeElementShowing("users");
    }
  }

  async changeElementShowing(newElement: string) {
    if (newElement == this.elementShowing) {
      return;
    }
    this.elementShowing = newElement;
    switch (this.elementShowing) {
      case "users":
        await this.getAllUsers();
        break;
      case "products":
        await this.getAllProducts();
        break;
    }
  }

  showEditRoleForm(user: User) {
    this.formState = "editRole";
    this.editRoleValue = user.role;
    this.selectedUser = user
  }

  showCreateProductForm() {
    this.formState = "createProduct";
    this.newProductName = "";
    this.newProductPrice = 0;
    this.newProductStock = 0;
    this.newProductCategory = "";
    this.newproductDescription = "";
    this.idToUpdate = 0
    this.create = true;
    this.image = null;
  }
  showEditProductForm(id: number){
    const translatepipe=new TranslatePipe();
    this.formState="modifyProduct"
    this.Product=this.allProducts[id-1];
    this.newProductName = this.Product.name;
    const convert=this.decimalPipe.transform(this.Product.price/100,"1.2-2");
    console.log(convert);
    if(convert==null){
      return;
    }
    this.pricedecimal=convert
    this.newProductPrice = parseFloat(this.pricedecimal);
    this.category=this.Product.category.name;
    this.categorytranslate=translatepipe.transform(this.category)
    this.newProductCategory = this.categorytranslate;
    this.newProductStock=this.Product.stock;
    this.newproductDescription=this.Product.description;
    this.idToUpdate = id
    this.create = false;
    this.image = null;
  }

  closeForm() {
    this.formState = null;
    this.image = null;
  }

  async submitEditRole() {
    if(this.selectedUser)
    {
      this.selectedUser.role = this.editRoleValue;
      await this.userService.updateUser(this.selectedUser);
      this.closeForm();
      this.getAllUsers();
      alert(`Rol actualizado a: ${this.editRoleValue}`);
    }
  }

  async submitCreateProduct() {
    //alert(`Producto creado: ${this.newProductName}, Precio: ${this.newProductPrice}, Categoría: ${this.newProductCategory}`);
    if(this.newProductName && this.newproductDescription && this.newProductPrice >= 0.5 && this.newProductStock >= 0 && this.newProductCategory)
    {
      if(this.create && this.image == null)
      {
        alert("No puedes insertar un producto sin una imagen")
        return;
      }

      // TODO: Cambiar ID de la categoría
      const newProduct = new ProductToInsert(
        this.image, this.newProductName, this.newproductDescription, this.newProductPrice * 100, this.newProductStock, this.newProductCategory, this.idToUpdate
      )

      console.log("NUEVO PRODUCTO MAMAHUEVO: ", newProduct)

      if(this.create)
      {
        await this.productService.createProduct(newProduct)
      }
      else
      {
        await this.productService.updateProduct(newProduct)
      }

      alert("Producto subido correctamente")
      
      await this.getAllProducts()
      this.closeForm();
    }
    else
    {
      alert("El precio debe ser mayor a 0,50 y todos los campos deben estar rellenos")
    }
  }
  /*async submitModifyProduct() {
    alert(`Producto creado: ${this.newProductName}, Precio: ${this.newProductPrice}, Categoría: ${this.newProductCategory}`);
    await this.productService.modifyProduct();
    this.closeForm();
  }*/

  async deleteUser(id: number) {
    const response = confirm("¿Seguro que quieres borrar al usuario?");
    if (response) {
      await this.userService.deleteUser(id);
      alert("Usuario borrado correctamente");
      this.getAllUsers();
    }
  }

  onFileSelected(event: any) {
    const image = event.target.files[0] as File;
    if(image)
    {
      console.log("NUEVA IMAGEN")
      this.image = image
    }
    else
    {
      console.log("NO HAY IMAGEN")
    }
  }

  async getAllUsers() {
    const users = await this.userService.getAllUsers();
    if (users != null) this.allUsers = users;
  }

  async getAllProducts() {
    const products = await this.productService.getCompleteProducts();
    if (products != null) this.allProducts = products;
  }

}

