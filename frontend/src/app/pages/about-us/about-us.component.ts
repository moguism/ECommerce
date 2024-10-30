import { Component, HostListener } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { Member } from '../../models/member';

@Component({
  selector: 'app-about-us',
  standalone: true,
  imports: [HeaderComponent],
  templateUrl: './about-us.component.html',
  styleUrl: './about-us.component.css'
})
export class AboutUsComponent {
  readonly position : number = 1500
  setUp : boolean = false
  readonly teamMembers : Member[] = []

  @HostListener('window:scroll', [])
  checkScroll() {
    const currentScroll = window.scrollY;

    if(currentScroll >= this.position && !this.setUp)
    {
      document.getElementById("test")?.classList.add("test")
      this.setUp = true;
    }
    console.log(currentScroll)
  }
}
