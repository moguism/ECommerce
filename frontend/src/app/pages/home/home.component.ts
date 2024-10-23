import { Component, HostListener } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [HeaderComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  showText = false;
  private scrollPosition = 1544;

  @HostListener('window:scroll', [])
  checkScroll()
  {
    const currentScroll = window.scrollY;

    if(currentScroll == this.scrollPosition)
    {
      document.getElementById("animatedSection")?.classList.add("mondongo");
    }

    console.log(currentScroll)
  }
}
