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

    if(currentScroll >= 800 && currentScroll <= 1500)
    {
      document.getElementById("second-animated-section")?.classList.add("animation-class");
    }
    else if(currentScroll >= 1500 && currentScroll <= 3000)
    {
      document.getElementById("third-animated-section")?.classList.add("animation-class");
    }

    if(currentScroll > 2500){
      document.getElementById("centered-image")?.classList.remove("centered-image");
      document.getElementById("centered-image")?.classList.add("centered-image-stop");
    }else{
      document.getElementById("centered-image")?.classList.add("centered-image");
      document.getElementById("centered-image")?.classList.remove("centered-image-stop");
    }
    //console.log(currentScroll)
  }
  text_activate(){
      document.getElementById("buy-text")?.classList.add("activate-text");
      document.getElementById("buy-text")?.classList.remove("defuse-text");
  }

  text_defuse(){
      document.getElementById("buy-text")?.classList.add("defuse-text");
      document.getElementById("buy-text")?.classList.remove("activate-text");
  }
}