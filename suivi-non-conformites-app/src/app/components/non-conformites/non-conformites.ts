import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  NonConformitesService,
  NonConformite,
  Gravite,
  StatutNonConformite,
} from '../../services/non-conformites';

@Component({
  selector: 'app-non-conformites',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './non-conformites.html',
  styleUrl: './non-conformites.css',
})
export class NonConformitesComponent implements OnInit {
  private ncService = inject(NonConformitesService);

  nonConformites = signal<NonConformite[]>([]);

  // Expose les enums au template (nécessaire, un template ne peut pas importer directement un enum)
  Gravite = Gravite;
  StatutNonConformite = StatutNonConformite;
  //Point TypeScript/Angular à bien retenir : un fichier .html (le template) ne peut pas importer quoi que ce soit directement — il ne peut lire que les propriétés de la classe du composant. Donc pour utiliser l'enum dans le HTML (ex: générer les options d'un <select>), il faut le réexposer comme une propriété de la classe, même si ça semble redondant à l'écriture.

  filtreStatut: StatutNonConformite | 'toutes' = 'toutes';
  colonneTri: 'gravite' | 'dateEcheance' = 'dateEcheance';
  triAscendant = true;

  nouvelleNc: Partial<NonConformite> = this.ncVide();

  creerNonConformite() {
    this.ncService.create(this.nouvelleNc).subscribe({
      next: () => {
        this.chargerNonConformites();
        this.nouvelleNc = this.ncVide();
      },
      error: (err) => console.error('Erreur création non-conformité', err),
    });
  }

  private ncVide(): Partial<NonConformite> {
    return { description: '', gravite: Gravite.Mineure, dateEcheance: '', auditId: undefined };
  }

  ngOnInit() {
    this.chargerNonConformites();
  }

  chargerNonConformites() {
    const statutFiltre = this.filtreStatut === 'toutes' ? undefined : this.filtreStatut;

    this.ncService.getAll(statutFiltre).subscribe({
      next: (data) => this.nonConformites.set(this.trier(data)),
      error: (err) => console.error('Erreur chargement non-conformités', err),
    });
  } //Ici, le filtre est côté serveur — changer filtreStatut puis rappeler chargerNonConformites() déclenche une nouvelle requête HTTP avec le paramètre ?statut=..., plutôt que de filtrer un tableau déjà en mémoire. C'est le choix le plus juste architecturalement (le filtrage se fait en base, pas sur des données déjà transférées), même si pour un petit jeu de données, filtrer côté client aurait aussi fonctionné.

  changerColonneTri(colonne: 'gravite' | 'dateEcheance') {
    if (this.colonneTri === colonne) {
      this.triAscendant = !this.triAscendant;
    } else {
      this.colonneTri = colonne;
      this.triAscendant = true;
    }
    this.nonConformites.set(this.trier(this.nonConformites()));
  }

  private trier(liste: NonConformite[]): NonConformite[] {
    const copie = [...liste];
    copie.sort((a, b) => {
      let comparaison = 0;
      if (this.colonneTri === 'gravite') {
        comparaison = a.gravite - b.gravite;
      } else {
        comparaison = new Date(a.dateEcheance).getTime() - new Date(b.dateEcheance).getTime();
      }
      return this.triAscendant ? comparaison : -comparaison;
    });
    return copie;
  } //[...liste] : copie le tableau avant de le trier — .sort() en JavaScript modifie le tableau original en place, ce qui casserait la logique du signal si on triait directement nonConformites() sans copie préalable (Angular ne détecterait pas forcément le changement correctement, et on risquerait des effets de bord).
  //.sort((a, b) => ...) : fonction de comparaison classique JS — retourne un nombre négatif, nul, ou positif pour indiquer l'ordre relatif de a et b.
  //Le choix de trier côté client ici (plutôt que côté serveur comme le filtre) est un choix pédagogique délibéré : ça te permet de manipuler .sort() et de voir la différence entre les deux approches, utile pour comprendre plus tard où placer chaque type de logique selon le contexte (volumes de données, fréquence de changement, etc.).

  changerStatut(nc: NonConformite, nouveauStatut: StatutNonConformite) {
    const ncMaj: NonConformite = { ...nc, statut: nouveauStatut };
    this.ncService.update(nc.id, ncMaj).subscribe({
      next: () => this.chargerNonConformites(),
      error: (err) => console.error('Erreur changement statut', err),
    });
  } //Plutôt qu'un formulaire séparé comme au projet 1, ici le changement de statut se fera directement via un <select> dans chaque ligne du tableau (on le voit dans le HTML) — pattern différent, plus adapté à ce cas d'usage (changement rapide d'un seul champ, sans rouvrir un formulaire complet).

  supprimerNonConformite(id: number) {
    this.ncService.delete(id).subscribe({
      next: () => this.chargerNonConformites(),
      error: (err) => console.error('Erreur suppression', err),
    });
  }
}
