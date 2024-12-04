import { Component, ViewChild, ElementRef } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { Member } from '../../models/member';
import * as THREE from 'three';

@Component({
  selector: 'app-about-us',
  standalone: true,
  imports: [HeaderComponent],
  templateUrl: './about-us.component.html',
  styleUrl: './about-us.component.css'
})
export class AboutUsComponent {
  @ViewChild('rendererContainer')
  rendererContainer!: ElementRef;

  renderer = new THREE.WebGLRenderer();
    scene ;
    camera;
    mesh;


  constructor() {
      this.scene = new THREE.Scene();

      this.camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 1, 10000);
      this.camera.position.z = 1000;

      const geometry = new THREE.BoxGeometry(200, 200, 200);
      const material = new THREE.MeshBasicMaterial({color: 0xff0000, wireframe: true});
      this.mesh = new THREE.Mesh(geometry, material);

      this.scene.add(this.mesh);
  }
  ngAfterViewInit() {
    this.renderer.setSize(window.innerWidth, window.innerHeight);
    this.rendererContainer.nativeElement.appendChild(this.renderer.domElement);
    this.animate();
}

animate() {
    window.requestAnimationFrame(() => this.animate());
    this.mesh.rotation.x += 0.01;
    this.mesh.rotation.y += 0.02;
    this.renderer.render(this.scene, this.camera);
}
}
