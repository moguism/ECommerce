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
  readonly teamMembers : Member[] = [
    new Member("assets/aboutus_images/mauricio.png", "MAURICIO", "Mauricio es una de las fuerzas que mueve a la granja. Le encanta participar en el cultivo de productos orgánicos y en la educación sobre prácticas agrícolas sostenibles."),
    new Member("assets/aboutus_images/daniel.png", "DANIEL", "Daniel cuenta con varios años de experiencia como director comercial en el sector agrícola. Le encantan los desafíos y ha ayudado a la granja a crecer y prosperar."),
    new Member("assets/aboutus_images/alejandro.png", "ALEJANDRO", "Alejandro es una de las personas icónicas en la empresa. Él es mentor de nuevos agricultores y se ocupa de la comunidad agrícola local."),
    new Member("assets/aboutus_images/francisco.png", "FRAN", "Francisco Siles tiene una amplia experiencia en agricultura sostenible. Su objetivo es garantizar la calidad y el éxito de nuestra granja.")
  ]

  @HostListener('window:scroll', [])
  checkScroll() {
    const currentScroll = window.scrollY;

    if(currentScroll >= this.position && !this.setUp)
    {
      document.getElementById("test")?.classList.add("test")
      this.setUp = true;
    }
    //console.log(currentScroll)
  }
}
