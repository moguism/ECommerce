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
export class HeaderComponent {
  protected buttonChange: boolean = true;
  
  showMenu() {
    this.buttonChange = false;
    const redElements = document.getElementsByClassName("red");
    const textElements = document.querySelectorAll(".text");
    const blackDiv = document.getElementById("black");
    
    for (let i = 0; i < redElements.length; i++) {
      redElements[i].className = "redVisible";
    }
    
    textElements.forEach((text) => {
      text.className = "textVisible";
    });
    
    if (blackDiv) {
      blackDiv.id = "blackVisible";
    }
  }

  closeMenu() {
    this.buttonChange = true;
    const redVisibleElements = document.getElementsByClassName("redVisible");
    const textVisibleElements = document.querySelectorAll(".textVisible");
    const blackVisibleDiv = document.getElementById("blackVisible");
    const screenWidth = window.innerWidth;

    if (screenWidth > 400) {
      for (let i = 0; i < redVisibleElements.length; i++) {
        redVisibleElements[i].className = "red";
      }
      textVisibleElements.forEach((textVisible) => {
        textVisible.className = "text";
      });
      if (blackVisibleDiv) {
        blackVisibleDiv.id = "black";
      }
    }
    
    for (let i = 0; i < redVisibleElements.length; i++) {
      redVisibleElements[i].className = "red";
    }
    textVisibleElements.forEach((textVisible) => {
      textVisible.className = "text";
    });
    
    if (blackVisibleDiv) {
      blackVisibleDiv.id = "black";
    }
  }

  @HostListener('window:resize')
  onResize() {
    this.closeMenu();
  }
}
