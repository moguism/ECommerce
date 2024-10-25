import { Component, HostListener } from '@angular/core';
import { LoginComponent } from '../../pages/login/login.component';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [LoginComponent, RouterModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent  {
  protected cambiodeboton: boolean=true;
  funcionmostramenu(){
    this.cambiodeboton=false;
    const rojo=document.getElementsByClassName("rojo");
    const textos = document.querySelectorAll(".texto");
    const negro=document.getElementById("negro");
    for (let i = 0; i < rojo.length; i++) {
      rojo[i].className = "rojovisible";
    }
    textos.forEach((texto) => {
      texto.className = "textovisible";
  });
    if(negro){
      negro.id="negrovisible"
    }
  }
  funcioncerrarmenu(){
    this.cambiodeboton=true;
    const rojovisible=document.getElementsByClassName("rojovisible");
    const textosvisibles = document.querySelectorAll(".textovisible");
    const negrovisible=document.getElementById("negrovisible");
    const anchoPantalla = window.innerWidth;
    if(anchoPantalla>400){
      for (let i = 0; i < rojovisible.length; i++) {
        rojovisible[i].className = "rojo";
      }
      textosvisibles.forEach((textosvisible) => {
        textosvisible.className = "texto";
    });
      if(negrovisible){
        negrovisible.id="negro"
      }
  }
  for (let i = 0; i < rojovisible.length; i++) {
    rojovisible[i].className = "rojo";
  }
  textosvisibles.forEach((textosvisible) => {
    textosvisible.className = "texto";
});
  if(negrovisible){
    negrovisible.id="negro"
  }
  }
  @HostListener('window:resize')
  onResize() {
    this.funcioncerrarmenu();
  }
}
