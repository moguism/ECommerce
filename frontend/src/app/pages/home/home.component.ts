import { Component, HostListener } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [HeaderComponent, RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  @HostListener('window:scroll', [])
  checkScroll()
  {
    const currentScroll = window.scrollY;
    const truck = document.getElementById("centered-image"); // Con esto se gestiona como de llena está la furgoneta
    if(currentScroll < 300){
      truck?.classList.remove("centered-image-2");
      truck?.classList.add("centered-image");
    }
    else if(currentScroll >= 300 && currentScroll < 800){
      truck?.classList.remove("centered-image");
      truck?.classList.add("centered-image-2");
      truck?.classList.remove("centered-image-3");
    }
    else if(currentScroll >= 800 && currentScroll <= 1500)
    {
      document.getElementById("second-animated-section")?.classList.add("animation-class");
      truck?.classList.remove("centered-image-2");
      truck?.classList.add("centered-image-3");
      truck?.classList.remove("centered-image-4");

    }
    else if(currentScroll >= 1500 && currentScroll <= 2000)
    {
      document.getElementById("third-animated-section")?.classList.add("animation-class");
      truck?.classList.remove("centered-image-3");
      truck?.classList.add("centered-image-4");
      truck?.classList.remove("centered-image-5");

    }else if(currentScroll > 2000 && currentScroll <= 2500){
      truck?.classList.remove("centered-image-4");
      truck?.classList.add("centered-image-5");
      truck?.classList.remove("centered-image-stop");

    }else if(currentScroll > 2500){
      truck?.classList.remove("centered-image-5");
      truck?.classList.add("centered-image-stop");
    }

    /*if(currentScroll > 2500){
      document.getElementById("centered-image")?.classList.remove("centered-image");
      document.getElementById("centered-image")?.classList.add("centered-image-stop");
    }else{
      document.getElementById("centered-image")?.classList.add("centered-image");
      document.getElementById("centered-image")?.classList.remove("centered-image-stop");
    }*/
    //console.log(currentScroll)
  }
  /*text_activate(){
      document.getElementById("buy-text")?.classList.add("activate-text");
      document.getElementById("buy-text")?.classList.remove("defuse-text");
  }

  text_defuse(){
      document.getElementById("buy-text")?.classList.add("defuse-text");
      document.getElementById("buy-text")?.classList.remove("activate-text");
  }*/
}